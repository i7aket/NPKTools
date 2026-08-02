// The only JavaScript in the app, and only for the two things .NET cannot reach from WebAssembly:
// local storage and the clipboard. Everything else — the calculation, the link, reading a picked file —
// is managed code.

const KEY = 'npktools.state.v1';

export function load() {
    try {
        return localStorage.getItem(KEY);
    } catch {
        // Private-browsing modes and blocked site data throw rather than returning null. Losing the
        // saved state is a inconvenience; failing to start the app over it would not be.
        return null;
    }
}

export function save(json) {
    try {
        localStorage.setItem(KEY, json);
        return true;
    } catch {
        return false;
    }
}

export function clear() {
    try {
        localStorage.removeItem(KEY);
    } catch {
        // Nothing to do: it is already unreachable.
    }
}

export async function copy(text) {
    try {
        await navigator.clipboard.writeText(text);
        return true;
    } catch {
        // Denied permission, or an insecure origin. The caller shows the link so it can be copied by
        // hand, so a false here is a nudge rather than a failure.
        return false;
    }
}
