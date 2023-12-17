import Link from './components/Link';
import Route from './components/Route';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import ConfirmPage from './pages/ConfirmPage';
import MainContentPage from './pages/MainContentPage';
import './App.css';
import { useState, useEffect } from 'react';
import axios from 'axios';
import useNavigation from './hooks/use-navigation';
import { gapi } from 'gapi-script';

function App() {
	const [errors, setErrors] = useState({});
	const { navigate } = useNavigation();
	const clientId = '238617088969-sbq9rl49dhr623f55j6ae2c5g32r6sqk.apps.googleusercontent.com';
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
		const response = await axios.post('https://localhost:7154/login', {
			email,
			password,
		});
		const accessToken = response.data.tokenType + ' ' + response.data.accessToken;
		console.log(response.data);
		console.log(accessToken);
		const iscConfirmResponse = await axios.get('https://localhost:7154/manage/info', {
			headers: {
				accept: 'application/json',
				Authorization: accessToken,
			},
		});
		setEmailConfirmation(iscConfirmResponse.data.isEmailConfirmed);
		console.log(iscConfirmResponse.data.isEmailConfirmed);
		emailConfirmation ? navigate('/confirm') : navigate('/all-habits');
	};

	const googleLogin = async res => {
		const email = res.profileObj.email;
		const password = '';
		const response = await axios.post('https://localhost:7154/register', {
			email,
			password,
		});
		console.log(res.data);
	};

	useEffect(() => {
		function start() {
			gapi.client.init({
				clientId: clientId,
				scope: 'email',
			});
		}
		gapi.load('client:auth2', start);
	});

	return (
		<>
			<Route path='/signin'>
				<LoginPage onLogin={login} onGoogleLogin={googleLogin} />
			</Route>
			<Route path='/signup'>
				<RegisterPage onRegister={register} errors={errors} />
			</Route>
			<Route path='/confirm'>
				<ConfirmPage />
			</Route>
			<Route path='/all-habits'>
				<MainContentPage />
			</Route>
		</>
	);
}

export default App;
