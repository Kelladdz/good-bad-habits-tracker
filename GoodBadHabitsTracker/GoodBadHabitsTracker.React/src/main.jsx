import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App.jsx';
import NavigationProvider from './context/NavigationContext.jsx';
import './index.css';

ReactDOM.createRoot(document.getElementById('root')).render(
	<NavigationProvider>
		<App />
	</NavigationProvider>
);
