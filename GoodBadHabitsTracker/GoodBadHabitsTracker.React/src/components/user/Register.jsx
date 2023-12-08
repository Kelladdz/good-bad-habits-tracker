import css from './user.module.css';
import Logo from '../../assets/logo.svg';
import User from '../../assets/user.svg';
import Password from '../../assets/password.svg';
import { Button } from 'react-bootstrap';
import Link from '../Link';

export default function Register() {
	const handleSubmit = event => {
		event.preventDefault();
	};

	return (
		<div>
			<div className={css['login-container']}>
				<div className='d-flex flex-column align-items-center'>
					<img className='mt-5 mb-3 w-50' src={Logo}></img>
					<p className={css['welcome-text']}>I'm glad you took matters into your own hands!</p>
					<p className={css['sign-in-text']}>Sign up</p>
					<form onSubmit={handleSubmit}>
						<div className={css['input-box']}>
							<img className={css['user-icon']} src={User}></img>
							<input className={css['input-field']} placeholder='E-mail' />
						</div>
						<div className={css['input-box']}>
							<img className={css['user-icon']} src={Password}></img>
							<input className={css['input-field']} type='password' placeholder='Password' />
						</div>
						<Button className={css['submit-btn']}>Register</Button>
						<div className={css['register-btn']}>
							<Link to='/signin'>Back</Link>
						</div>
					</form>
				</div>
			</div>
		</div>
	);
}
