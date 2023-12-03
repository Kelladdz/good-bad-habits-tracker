import css from './login.module.css';
import Logo from '../../assets/logo.svg';
import User from '../../assets/user.svg';
import Password from '../../assets/password.svg';
import Google from '../../assets/google.svg';
import Facebook from '../../assets/facebook.svg';
import { Button } from 'react-bootstrap';

export default function Login() {
	const handleSubmit = event => {
		event.preventDefault();
	};

	return (
		<div>
			<div className={css['container']}>
				<div className='d-flex flex-column align-items-center'>
					<img className='mt-5 mb-3 w-50' src={Logo}></img>
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
						<Button>Login</Button>
						<Button>Register</Button>
					</form>

					<div className={css['or-with-box']}>
						<div className={css['line']}></div>
						<span>or with</span>
						<div className={css['line']}></div>
					</div>

					<div className='d-flex justify-content-between align-center mt-4 w-50'>
						<img className={css['external-icon']} src={Google}></img>
						<img className={css['external-icon']} src={Facebook}></img>
					</div>
				</div>
			</div>
		</div>
	);
}
