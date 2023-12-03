import Link from './components/Link';
import Route from './components/Route';
import LoginPage from './pages/LoginPage';
import './App.css';
import { useState } from 'react';

function App() {
	return (
		<Route path='/signin'>
			<LoginPage />
		</Route>
	);
}

export default App;
