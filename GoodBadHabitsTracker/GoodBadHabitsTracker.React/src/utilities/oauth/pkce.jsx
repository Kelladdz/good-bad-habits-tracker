export const Pkce = {
    codeVerifierGenerator: function() {
        let codeVerifier = '';
        const possible = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
        const length = Math.floor(Math.random() * (128 - 43 + 1)) + 43;
        for (let i = 0; i < length; i++) {
            codeVerifier += possible.charAt(Math.floor(Math.random() * possible.length));
        }
        return codeVerifier;
    },

    codeChallengeGeneratorAsync: async function(codeVerifier) {
        const encoder = new TextEncoder();
        const data = encoder.encode(codeVerifier);
        const hash = await window.crypto.subtle.digest('SHA-256', data);
        const base64EncodedHash = btoa(String.fromCharCode.apply(null, new Uint8Array(hash)));
        const codeChallenge = base64EncodedHash.replace(/=/g, '').replace(/\+/g, '-').replace(/\//g, '_');
        return codeChallenge;
    },

    stateParameterGenerator: function() {
        let stateParameter = '';
        const possible = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
        const length = Math.floor(Math.random() * (30 - 20 + 1)) + 20;
        for (let i = 0; i < length; i++) {
            stateParameter += possible.charAt(Math.floor(Math.random() * possible.length));
        }
        return stateParameter;
    }
}