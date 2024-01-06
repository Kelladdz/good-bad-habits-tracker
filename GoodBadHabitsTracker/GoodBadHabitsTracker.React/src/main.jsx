import React, { StrictMode } from 'react';
import ReactDOM from 'react-dom/client';
import App from './App.jsx';
import { NavigationProvider } from './context/navigation.jsx';
import './index.css';
import { CookiesProvider } from 'react-cookie';
import LoginProvProvider from './context/loginProvider.jsx';

ReactDOM.createRoot(document.getElementById('root')).render(
	<StrictMode>
		<CookiesProvider>
			<NavigationProvider>
				<App />
			</NavigationProvider>
		</CookiesProvider>
	</StrictMode>
);
