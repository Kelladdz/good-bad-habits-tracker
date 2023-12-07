import css from './user.module.css';
import Logo from '../../assets/logo.svg';
import User from '../../assets/user.svg';
import Password from '../../assets/password.svg';
import Google from '../../assets/google.svg';
import Facebook from '../../assets/facebook.svg';
import { Button } from 'react-bootstrap';
import Link from '../Link';

export default function Login() {
	const welcomeText = document.querySelector('.welcome-text');
	const maxZoom = 0.1;
	let zoom = 1;

	const renderText = () => {
		welcomeText.innerText = zoom;
	};

	const handleSubmit = event => {
		event.preventDefault();
	};

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
							<input className={css['input-field']} placeholder='E-mail' />
						</div>
						<div className={css['input-box']}>
							<img className={css['user-icon']} src={Password}></img>
							<input className={css['input-field']} type='password' placeholder='Password' />
						</div>
						<Button className={css['submit-btn']}>Login</Button>
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
						<a className={css['external-link']} href='#'>
							<img className={css['external-icon']} src={Google} />
						</a>
						<a className={css['external-link']} style={{ paddingLeft: '3rem' }} href='#'>
							<img className={css['external-icon']} src={Facebook} />
						</a>
					</div>
				</div>
			</div>
		</div>
	);
}
