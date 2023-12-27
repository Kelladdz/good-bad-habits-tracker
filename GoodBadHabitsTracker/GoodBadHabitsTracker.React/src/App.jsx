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
	const [registerServerErrors, setRegisterServerErrors] = useState('');
	const [loginErrors, setLoginErrors] = useState('');
	const [forgetPasswordErrors, setForgetPasswordErrors] = useState('');

	const { navigate } = useNavigation();

	// const clientId = '238617088969-sbq9rl49dhr623f55j6ae2c5g32r6sqk.apps.googleusercontent.com';

	// const googleLogin = async res => {
	// 	const email = res.profileObj.email;
	// 	const password = '';
	// 	const response = await axios.post('https://localhost:7154/register', {
	// 		email,
	// 		password,
	// 	});
	// 	console.log(res.data);
	// };

	const sendLinkForResetPassword = async email => {
		const response = await axios
			.post('https://localhost:7154/API/Account/ForgetPassword', {
				email,
			})
			.then(res => {
				console.log(res.data);
			})
			.catch(errs => {
				console.log(errs);
				let errorData = '';
				if (errs.response.status === 404) errorData = `This account doesn't exist`;
				setForgetPasswordErrors(errorData);
			});
	};

	const resetPassword = async (password, token, userId) => {
		const response = await axios.put('https://localhost:7154/API/Account/ResetPassword', {
			password,
			token,
			userId,
		});
	};

	const logout = async () => {
		const response = await axios
			.get('https://localhost:7154/API/Account/Logout', { withCredentials: true })
			.then(navigate('/'));
		navigate('/signin');
	};

	useEffect(() => {
		setRegisterErrors({});
		setLoginErrors('');
		setForgetPasswordErrors('');
		console.log(registerErrors, loginErrors, forgetPasswordErrors);
	}, [navigate]);

	return (
		<>
			<Route path='/'>
				<Home />
			</Route>
			<Route path='/signin'>
				{/* <LoginPage onLogin={login} 
				onGoogleLogin={googleLogin} /> */}
				<LoginPage />
			</Route>
			<Route path='/signup'>
				<RegisterPage/>
			</Route>
			<Route path='/all-habits'>
				<MainContentPage onLogout={logout} />
			</Route>
			<Route path='/forget-password'>
				<ForgetPasswordPage onPasswordForget={sendLinkForResetPassword} forgetPasswordErrors={forgetPasswordErrors} />
			</Route>
			<Route path='/reset-password'>
				<ResetPasswordPage onResetPassword={resetPassword} />
			</Route>
		</>
	);
}

export default App;
