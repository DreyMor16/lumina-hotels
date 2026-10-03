import { spawn } from "node:child_process";
import { access, mkdir, mkdtemp, rm, writeFile } from "node:fs/promises";
import { tmpdir } from "node:os";
import { join, resolve } from "node:path";
import { createServer } from "node:net";

const baseUrl = (process.env.LUMINA_BASE_URL || "http://localhost:51344").replace(/\/$/, "");
const adminUser = process.env.LUMINA_ADMIN_USER;
const adminPassword = process.env.LUMINA_ADMIN_PASSWORD;
const clientUser = process.env.LUMINA_CLIENT_USER;
const clientPassword = process.env.LUMINA_CLIENT_PASSWORD;
const outputDir = resolve(process.cwd(), "docs", "screenshots");

if (!adminUser || !adminPassword || !clientUser || !clientPassword) {
    throw new Error("Define LUMINA_ADMIN_USER, LUMINA_ADMIN_PASSWORD, LUMINA_CLIENT_USER y LUMINA_CLIENT_PASSWORD.");
}

const chromeCandidates = [
    process.env.CHROME_PATH,
    "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe",
    "C:\\Program Files (x86)\\Microsoft\\Edge\\Application\\msedge.exe"
].filter(Boolean);

let chromePath;
for (const candidate of chromeCandidates) {
    try {
        await access(candidate);
        chromePath = candidate;
        break;
    } catch { /* probar el siguiente navegador */ }
}
if (!chromePath) throw new Error("No se encontró Chrome o Edge. Define CHROME_PATH.");

async function freePort() {
    return await new Promise((resolvePort, reject) => {
        const server = createServer();
        server.once("error", reject);
        server.listen(0, "127.0.0.1", () => {
            const { port } = server.address();
            server.close(() => resolvePort(port));
        });
    });
}

class CdpClient {
    constructor(url) {
        this.socket = new WebSocket(url);
        this.sequence = 0;
        this.pending = new Map();
        this.listeners = new Map();
    }

    async open() {
        await new Promise((resolveOpen, reject) => {
            this.socket.addEventListener("open", resolveOpen, { once: true });
            this.socket.addEventListener("error", reject, { once: true });
        });
        this.socket.addEventListener("message", event => {
            const message = JSON.parse(event.data);
            if (message.id && this.pending.has(message.id)) {
                const { resolve: ok, reject } = this.pending.get(message.id);
                this.pending.delete(message.id);
                if (message.error) reject(new Error(message.error.message));
                else ok(message.result || {});
                return;
            }
            const waiters = this.listeners.get(message.method) || [];
            this.listeners.delete(message.method);
            waiters.forEach(waiter => waiter(message.params || {}));
        });
    }

    send(method, params = {}) {
        const id = ++this.sequence;
        return new Promise((resolveSend, reject) => {
            this.pending.set(id, { resolve: resolveSend, reject });
            this.socket.send(JSON.stringify({ id, method, params }));
        });
    }

    waitFor(method, timeoutMs = 15000) {
        return new Promise((resolveEvent, reject) => {
            const timer = setTimeout(() => reject(new Error(`Tiempo agotado esperando ${method}`)), timeoutMs);
            const done = params => {
                clearTimeout(timer);
                resolveEvent(params);
            };
            const waiters = this.listeners.get(method) || [];
            waiters.push(done);
            this.listeners.set(method, waiters);
        });
    }

    close() { this.socket.close(); }
}

const delay = milliseconds => new Promise(resolveDelay => setTimeout(resolveDelay, milliseconds));

async function waitForDebugger(port) {
    for (let attempt = 0; attempt < 40; attempt += 1) {
        try {
            const targets = await fetch(`http://127.0.0.1:${port}/json/list`).then(response => response.json());
            const page = targets.find(target => target.type === "page");
            if (page) return page.webSocketDebuggerUrl;
        } catch { /* Chrome todavía está iniciando */ }
        await delay(250);
    }
    throw new Error("Chrome no abrió el puerto de depuración.");
}

async function navigate(client, url) {
    const loaded = client.waitFor("Page.loadEventFired", 30000);
    await client.send("Page.navigate", { url });
    await loaded;
    await delay(450);
}

async function evaluate(client, expression) {
    const result = await client.send("Runtime.evaluate", { expression, returnByValue: true, awaitPromise: true });
    if (result.exceptionDetails) throw new Error(result.exceptionDetails.text || "Error evaluando la página");
    return result.result?.value;
}

async function login(client, username, password, expectedPath) {
    await navigate(client, `${baseUrl}/Login.aspx`);
    const loaded = client.waitFor("Page.loadEventFired", 30000);
    await evaluate(client, `(() => {
        const user = document.querySelector('#txtUserWeb');
        const pass = document.querySelector('#txtPassWeb');
        if (!user || !pass) throw new Error('No se encontró el formulario de acceso');
        user.value = ${JSON.stringify(username)};
        pass.value = ${JSON.stringify(password)};
        document.querySelector('#btnEntrarWeb').click();
        return true;
    })()`);
    await loaded;
    await delay(500);
    const currentUrl = await evaluate(client, "location.href");
    if (!currentUrl.includes(expectedPath)) throw new Error(`El acceso no llegó a ${expectedPath}: ${currentUrl}`);
}

async function capture(client, path, fileName) {
    await navigate(client, `${baseUrl}/${path}`);
    await evaluate(client, "window.scrollTo(0, 0); document.fonts ? document.fonts.ready.then(() => true) : true");
    await delay(450);
    const { data } = await client.send("Page.captureScreenshot", { format: "png", fromSurface: true, captureBeyondViewport: false });
    await writeFile(join(outputDir, fileName), Buffer.from(data, "base64"));
    const title = await evaluate(client, "document.title");
    console.log(`✓ ${fileName} — ${title}`);
}

await mkdir(outputDir, { recursive: true });
const port = await freePort();
const profileDir = await mkdtemp(join(tmpdir(), "lumina-captures-"));
const browser = spawn(chromePath, [
    "--headless=new",
    "--disable-gpu",
    "--hide-scrollbars",
    "--no-first-run",
    "--no-default-browser-check",
    `--remote-debugging-port=${port}`,
    `--user-data-dir=${profileDir}`,
    "about:blank"
], { stdio: "ignore" });

let client;
try {
    const socketUrl = await waitForDebugger(port);
    client = new CdpClient(socketUrl);
    await client.open();
    await client.send("Page.enable");
    await client.send("Runtime.enable");
    await client.send("Network.enable");
    await client.send("Emulation.setDeviceMetricsOverride", { width: 1600, height: 1000, deviceScaleFactor: 1, mobile: false });

    await client.send("Network.clearBrowserCookies");
    await capture(client, "Default.aspx", "01-inicio-publico.png");
    await capture(client, "Login.aspx", "02-acceso-unico.png");
    await capture(client, "RegistroCliente.aspx", "03-registro-cliente.png");

    await login(client, clientUser, clientPassword, "PerfilCliente.aspx");
    await capture(client, "PerfilCliente.aspx", "04-portal-cliente.png");
    await capture(client, "HacerReserva.aspx", "05-busqueda-habitaciones.png");
    await capture(client, "MisReservas.aspx", "06-mis-reservas.png");
    await capture(client, "HistorialReservas.aspx", "07-historial-estancias.png");

    await client.send("Network.clearBrowserCookies");
    await login(client, adminUser, adminPassword, "AdminDashboard.aspx");
    await capture(client, "AdminDashboard.aspx", "08-panel-administrativo.png");
    await capture(client, "AdminReservas.aspx", "09-reservaciones-admin.png");
} finally {
    if (client) client.close();
    browser.kill();
    await delay(300);
    await rm(profileDir, { recursive: true, force: true });
}
