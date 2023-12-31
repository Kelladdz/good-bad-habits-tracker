import css from './user.module.css';
import Logo from '../../assets/logo.svg';
import User from '../../assets/user.svg';
import Password from '../../assets/password.svg';
import Google from '../../assets/google.svg';
import Facebook from '../../assets/facebook.svg';
import { Button } from 'react-bootstrap';
import Link from '../Link';
import { useState, useEffect, useContext } from 'react';
import { GoogleLogin, onFailure } from 'react-google-login';
import { gapi } from 'gapi-script';
import useNavigation from '../../hooks/useNavigation';
import Cookies from 'js-cookie';

export default function Login() {
	const [email, setEmail] = useState('');
	const [password, setPassword] = useState('');
	const [loginErrors, setLoginErrors] = useState('');
	const { navigate } = useNavigation();

	const clientId = '238617088969-sbq9rl49dhr623f55j6ae2c5g32r6sqk.apps.googleusercontent.com';

	const login = async (email, password) => {
		let errorData = '';
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
				console.log(res);
				if (res.status === 200) navigate('/all-habits');
			})
			.catch(errs => {
				console.log(errs);
				if (errs.response.status === 401 || errs.response.data.includes('NullReferenceException'))
					errorData = 'Invalid email or password';
			});
		setLoginErrors(errorData);
	};

	const googleLogin = res => {
		const imageUrl = res.profileObj.imageUrl;
		const email = res.profileObj.email;
		const name = res.profileObj.name;
		console.log(imageUrl);
		console.log(email);
		console.log(name);
		window.open('https://localhost:7154/API/Account/GoogleLogin?provider=Google', '_self');
		// const response = await axios
		// 	// .post('https://localhost:7154/API/Account/GoogleLogin', {
		// 	// 	imageUrl,
		// 	// 	email,
		// 	// 	name,
		// 	// })
		// 	.post('https://localhost:7154/API/Account/GoogleLogin?provider=Google')
		// 	.then(resp => {
		// 		console.log(resp);
		// 		if (resp.status === 204) {
		// 			console.log('204');
		// 			navigate('/');
		// 		}
		// 	})
		// 	.catch(err => {
		// 		console.log(err);
		// 		if (err.response.status === 400) console.log('400');
		// 	});
		console.log(res.profileObj);
		console.log(res);
	};

	const handleSubmit = event => {
		event.preventDefault();
		login(email, password);
	};

	useEffect(() => {
		console.log(loginErrors);
		return setLoginErrors(loginErrors);
	}, [loginErrors]);

	useEffect(() => {
		function start() {
			gapi.client.init({
				clientId: clientId,
				scope: 'https://www.googleapis.com/auth/userinfo.email https://www.googleapis.com/auth/userinfo.profile',
			});
		}
		gapi.load('client:auth2', start);
	});

	useEffect(() => {
		const userCookie = () => {
			return Cookies.get('Logged');
		};
		console.log(userCookie());
		if (userCookie() !== undefined) {
			navigate('/all-habits');
		}
	});

	const handleChangeEmail = event => setEmail(event.target.value);
	const handleChangePassword = event => setPassword(event.target.value);

	return (
		<div>
			<div className={css['login-container']}>
				<div className='d-flex flex-column align-items-center relative'>
					<img className={css['logo']} src={Logo}></img>
					<p className={css['welcome-text']}>I'm glad you took matters into your own hands!</p>
					<p className={css['sign-in-text']}>Sign in</p>
					<form onSubmit={handleSubmit}>
						<div className={css['input-box']}>
							<img className={css['user-icon']} src={User}></img>
							<input className={css['input-field']} value={email} onChange={handleChangeEmail} placeholder='E-mail' />
						</div>
						<div className={css['input-box']}>
							<img className={css['user-icon']} src={Password}></img>
							<input
								className={css['input-field']}
								value={password}
								type='password'
								onChange={handleChangePassword}
								placeholder='Password'
							/>
						</div>
						<div className={css['error-box']}>
							{loginErrors && <p className={css['error-text']}>{loginErrors}</p>}
							<div className={css['forgot-password-btn']}>
								<Link to='/forget-password'>
									<span>Forget Password?</span>
								</Link>
							</div>
						</div>
						<Button className={css['submit-btn']} type='submit'>
							Login
						</Button>
						<div className={css['register-btn']}>
							<Link to='/signup'>Register</Link>
						</div>
					</form>

					<div className={css['or-with-box']}>
						<div className={css['line']}></div>
						<span>or</span>
						<div className={css['line']}></div>
					</div>
					<div className={css['icons']}>
						<div id='signInButton' className={css['sign-in-btn']}>
							<GoogleLogin
								clientId={clientId}
								onSuccess={googleLogin}
								onFailure={onFailure}
								cookiePolicy={'single_host_origin'}
							/>
							<img className={css['external-icon']} src={Google} />
						</div>
						<div id='signInButton' className={css['sign-in-btn']}>
							<a className={css['external-link']} style={{ paddingLeft: '3rem' }} href='#'>
								<img className={css['external-icon']} src={Facebook} />
							</a>
						</div>
					</div>
				</div>
			</div>
		</div>
	);
}
