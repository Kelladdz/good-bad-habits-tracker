import Link from './components/Link';
import Route from './components/Route';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import MainContentPage from './pages/MainContentPage';
import ForgetPasswordPage from './pages/ForgetPasswordPage';
import ResetPasswordPage from './pages/ResetPasswordPage';
import Home from './components/home/Home';
import './App.css';
import { useState, useEffect } from 'react';
import axios from 'axios';
import useNavigation from './hooks/useNavigation';
import Callback from './components/user/Callback';
import FacebookCallback from './components/user/FacebookCallback';
import { BrowserRouter as Router } from 'react-router-dom';

function App() {
	const [registerErrors, setRegisterErrors] = useState({});
	const [loginErrors, setLoginErrors] = useState('');
	const [forgetPasswordErrors, setForgetPasswordErrors] = useState('');
	const [loginProvider, setLoginProvider] = useState('');

	const { navigate } = useNavigation();

	// const googleLogin = async res => {
	// 	const email = res.profileObj.email;
	// 	const password = '';
	// 	const response = await axios.post('https://localhost:7154/API/Account/LoginGoogle', {
	// 		email,
	// 		password,
	// 	});
	// 	console.log(res.profileObj);
	// 	console.log(res.data);
	// };

	// const logout = () => {
	// 	window.open('https://localhost:7154/API/Account/GoogleLogout?provider=Google', '_self');
	// };

	return (
		<Router>
			<Route path='/'>
				<Home />
			</Route>
			<Route path='/signin'>
				<LoginPage />
			</Route>
			<Route path='/callback'>
				<Callback />
			</Route>
			<Route path='/callback-facebook'>
				<FacebookCallback />
			</Route>
			<Route path='/signup'>
				<RegisterPage />
			</Route>
			<Route path='/all-habits'>
				<MainContentPage />
			</Route>
			<Route path='/forget-password'>
				<ForgetPasswordPage />
			</Route>
			<Route path='/reset-password'>
				<ResetPasswordPage />
			</Route>
		</Router>
	);
}

export default App;
