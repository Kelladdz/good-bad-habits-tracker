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
	const [accessToken, setAccessToken] = useState();
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
			.then(navigate('/all-habits'));
		console.log(response.data);
		Cookies.set('UserLoginCookie', response.data);

		// if (response.status === 200) navigate('/all-habits');
		// const iscConfirmResponse = await axios.get('https://localhost:7154/manage/info', {
		// 	headers: {
		// 		accept: 'application/json',
		// 		Authorization: accessToken,
		// 	},
		// });
		// setEmailConfirmation(iscConfirmResponse.data.isEmailConfirmed);
		// console.log(iscConfirmResponse.data.isEmailConfirmed);
		// emailConfirmation ? navigate('/confirm') : navigate('/all-habits');
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

	return (
		<>
			<Route path='/'>
				<Home accessToken={accessToken} />
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
				<MainContentPage />
			</Route>
		</>
	);
}

export default App;
