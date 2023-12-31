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
import ResetPassword from './components/user/ResetPassword';

function App() {
	const [registerErrors, setRegisterErrors] = useState({});
	const [loginErrors, setLoginErrors] = useState('');
	const [forgetPasswordErrors, setForgetPasswordErrors] = useState('');

	const { navigate } = useNavigation();

	const clientId = '238617088969-sbq9rl49dhr623f55j6ae2c5g32r6sqk.apps.googleusercontent.com';

	const googleLogin = async res => {
		const email = res.profileObj.email;
		const password = '';
		const response = await axios.post('https://localhost:7154/API/Account/LoginGoogle', {
			email,
			password,
		});
		console.log(res.profileObj);
		console.log(res.data);
	};

	const logout = () => {
		window.open('https://localhost:7154/API/Account/GoogleLogout?provider=Google', '_self');
	};

	return (
		<>
			<Route path='/'>
				<Home />
			</Route>
			<Route path='/signin'>
				<LoginPage />
			</Route>
			<Route path='/signup'>
				<RegisterPage />
			</Route>
			<Route path='/all-habits'>
				<MainContentPage onLogout={logout} />
			</Route>
			<Route path='/forget-password'>
				<ForgetPasswordPage />
			</Route>
			<Route path='/reset-password'>
				<ResetPasswordPage />
			</Route>
		</>
	);
}

export default App;
