import { defineConfig } from 'vite';
import mkcert from 'vite-plugin-mkcert';
import { readFileSync } from 'fs';
import { resolve } from 'path';

export default defineConfig({
	server: {
		port: 8080,
		https: {
			key: readFileSync(resolve('C:\\Users\\Admin\\.vite-plugin-mkcert\\dev.pem')),
			cert: readFileSync(resolve('C:\\Users\\Admin\\.vite-plugin-mkcert\\cert.pem')), // Not needed for Vite 5+
		},
		cors: {
			origin: 'https://localhost:7154',
			optionsSuccessStatus: 200,
		},
	},
	plugins: [mkcert()],
});
