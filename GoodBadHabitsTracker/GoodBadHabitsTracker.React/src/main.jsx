import React, { StrictMode } from 'react';
import ReactDOM from 'react-dom/client';
import App from './App.jsx';
import { NavigationProvider } from './context/navigation.jsx';
import './index.css';
import { CookiesProvider } from 'react-cookie';
import { GoogleOAuthProvider } from '@react-oauth/google';

ReactDOM.createRoot(document.getElementById('root')).render(

		<GoogleOAuthProvider clientId='238617088969-sbq9rl49dhr623f55j6ae2c5g32r6sqk.apps.googleusercontent.com'>
			<CookiesProvider>
				<NavigationProvider>
					<App />
				</NavigationProvider>
			</CookiesProvider>
		</GoogleOAuthProvider>

);
