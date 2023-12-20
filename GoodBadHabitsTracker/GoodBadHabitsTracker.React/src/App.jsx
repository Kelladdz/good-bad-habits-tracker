import Link from './components/Link';
import Route from './components/Route';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import MainContentPage from './pages/MainContentPage';
import Home from './components/home/Home';
import './App.css';
import { useState, useEffect } from 'react';
import axios from 'axios';
import useNavigation from './hooks/useNavigation';
import { gapi } from 'gapi-script';
import Cookies from 'js-cookie';

function App() {
	const [errors, setErrors] = useState({});
	const { navigate } = useNavigation();
	// const clientId = '238617088969-sbq9rl49dhr623f55j6ae2c5g32r6sqk.apps.googleusercontent.com';
	const register = async (email, name, password, confirmPassword) => {
		const response = await axios
			.post('https://localhost:7154/API/Account/Register', {
				email,
				name,
				password,
				confirmPassword,
			})
			.catch(errs => {
				console.log(errs.response.data);
				const errorData = {};
				for (let err of errs.response.data) {
					if (err.code === 'DuplicateUserName') errorData.name = `Username ${name} is already taken.`;
					else if (err.code === 'DuplicateEmail') errorData.email = `Email ${email} is already taken.`;
				}
				console.log(errorData);
				setErrors(errorData);
			});
	};

	const login = async (email, password) => {
		const response = await axios
			.post(
				'https://localhost:7154/API/Account/Login',
				{
					email,
					password,
				},
				{ withCredentials: true }
			)
			.then(res => {
				if (res.status === 200) navigate('/all-habits');
			});
		console.log(response.data);
	};

	// const googleLogin = async res => {
	// 	const email = res.profileObj.email;
	// 	const password = '';
	// 	const response = await axios.post('https://localhost:7154/register', {
	// 		email,
	// 		password,
	// 	});
	// 	console.log(res.data);
	// };

	const logout = async () => {
		const response = await axios
			.get('https://localhost:7154/API/Account/Logout', { withCredentials: true })
			.then(navigate('/'));
	};

	useEffect(() => {
		const userCookie = () => {
			return Cookies.get('Logged');
		};
		if (userCookie() === undefined) {
			navigate('/signin');
		} else navigate('/all-habits');
	});

	return (
		<>
			<Route path='/'>
				<Home />
			</Route>
			<Route path='/signin'>
				{/* <LoginPage onLogin={login} 
				onGoogleLogin={googleLogin} /> */}
				<LoginPage onLogin={login} />
			</Route>
			<Route path='/signup'>
				<RegisterPage onRegister={register} errors={errors} />
			</Route>
			<Route path='/all-habits'>
				<MainContentPage onLogout={logout} />
			</Route>
		</>
	);
}

export default App;
