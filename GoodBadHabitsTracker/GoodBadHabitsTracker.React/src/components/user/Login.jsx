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

export default function Login({onLogin, onGoogleLogin}) {
	const [email, setEmail] = useState('');
	const [password, setPassword] = useState('');

	const clientId = "238617088969-cugrh0v8a7b0vfti8c42rjio7spms255.apps.googleusercontent.com";

	const handleSubmit = event => {
		event.preventDefault();
		onLogin(email, password);
	};

	const handleChangeEmail = event => setEmail(event.target.value);
	const handleChangePassword = event => setPassword(event.target.value);

	const onSuccess = (res) => console.log("Success!", res.profileObj);
    const onFailure = (res) => console.log("Fail!", res)

	useEffect(() => {
		function start() {
			gapi.client.init({
				clientId: clientId,
				scope: ""
			})
		};
		gapi.load('client:auth2', start);
	})

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
							<input className={css['input-field']} value={password} type='password' onChange={handleChangePassword} placeholder='Password' />
						</div>					
						<Button className={css['submit-btn']} type='submit'>Login</Button>
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
						<div id="signInButton">
            				<GoogleLogin 
                				clientId={clientId}
                				
                				onSuccess={onSuccess}
								onFailure={onFailure}
              					cookiePolicy={'single_host_origin'}
                				isSignedIn={true}
           					 />
							 <img className={css['external-icon']} src={Google} />
       					 </div>
						<a className={css['external-link']} style={{ paddingLeft: '3rem' }} href='#'>
							<img className={css['external-icon']} src={Facebook} />
						</a>
					</div>
				</div>
			</div>
		</div>
	);
}
