import css from './user.module.css';
import Logo from '../../assets/logo.svg';
import User from '../../assets/user.svg';
import Password from '../../assets/password.svg';
import Google from '../../assets/google.svg';
import Facebook from '../../assets/facebook.svg';
import { Button } from 'react-bootstrap';
import Link from '../Link';
import { useState, useEffect } from 'react';
import { GoogleLogin } from 'react-google-login';
import FacebookLogin from 'react-facebook-login';
import useNavigation from '../../hooks/useNavigation';
import Cookies from 'js-cookie';
import { Pkce } from '../../utilities/oauth/pkce';
import axios from 'axios';

export default function Login() {
	const [email, setEmail] = useState('');
	const [password, setPassword] = useState('');
	const [loginErrors, setLoginErrors] = useState('');
	const [cookie, setCookie] = useState(Cookies.get('ONSESS'));
	const { navigate } = useNavigation();

	const login = async (email, password) => {
		let errorData = '';
		const response = await axios
			.post(
				'https://localhost:7154/api/auth/login',
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

	const googleLogin = async res => {
		const clientId = 'cNRB11SQnB796najkgVTLftkwgkdtNL5';
		const redirectUri = 'https://localhost:8080/callback';
		const scope = 'openid+profile+email';
		const stateParameter = Pkce.stateParameterGenerator();
		console.log(stateParameter);
		const codeVerifier = Pkce.codeVerifierGenerator();
		console.log(codeVerifier);
		localStorage.setItem('codeVerifier', codeVerifier);
		const codeChallenge = await Pkce.codeChallengeGeneratorAsync(codeVerifier);
		console.log(codeChallenge);

		window.open(
			`https://dev-d3sgzf7qkeuvnndt.eu.auth0.com/authorize?response_type=code&client_id=${clientId}&connection=google-oauth2&redirect_uri=${redirectUri}&scope=${scope}&state=${stateParameter}&code_challenge=${codeChallenge}&code_challenge_method=S256`,
			'_blank',
			'width=500,height=600'
		);
		// window.open(`https://localhost:7154/api/auth/external-login?provider=Google`);
		// console.log(res);

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
		// 	})
	};

	const facebookLogin = res => {
		console.log(res);
		window.open('https://localhost:7154/api/auth/external-login?provider=Facebook', '_blank');
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
		const userCookie = () => {
			return Cookies.get('ONSESS');
		};
		console.log(userCookie());
		if (userCookie() !== undefined) {
			navigate('/all-habits');
		}
	});

	useEffect(() => {
		const checkCookie = () => {
			const currentCookie = Cookies.get('ONSESS');
			if (currentCookie !== cookie) {
				setCookie(currentCookie);
			}
		};

		// Sprawdzanie ciasteczka co sekundę
		const intervalId = setInterval(checkCookie, 1000);

		// Czyszczenie interwału po odmontowaniu komponentu
		return () => clearInterval(intervalId);
	}, [cookie]);

	// useEffect(() => {
	// 	google.accounts.id.initialize({
	// 		client_id: '238617088969-sbq9rl49dhr623f55j6ae2c5g32r6sqk.apps.googleusercontent.com',
	// 		callback: googleLogin,
	// 		auto_select: true,
	// 		ui_mode: 'card',
	// 	});
	// 	google.accounts.id.renderButton(document.getElementById('signInButton'), {
	// 		theme: 'filled_black',
	// 		size: 'large',
	// 		type: 'standard',
	// 		text: 'continue_with',
	// 		shape: 'rectangular',
	// 		prompt_parent_id: 'googleSignInButton',
	// 	});
	// }, []);

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
						<a href='#' className={css['sign-in-btn']} onClick={googleLogin}>
							<img className={css['external-icon']} src={Google} />
						</a>
						<div id='signInButton' className={css['sign-in-btn']}>
							{/* <FacebookLogin
								appId='326877753548738'
								scope='email, public_profile'
								autoLoad={false}
								fields='name,email,picture'
								callback={facebookLogin}
							/> */}

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
