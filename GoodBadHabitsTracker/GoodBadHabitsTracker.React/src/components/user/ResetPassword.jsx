import { useEffect, useState } from 'react';
import css from './user.module.css';
import Logo from '../../assets/logo.svg';
import Password from '../../assets/password.svg';
import { Button } from 'react-bootstrap';

export default function ResetPassword({ onResetPassword }) {
	const [password, setPassword] = useState('');
	const [confirmPassword, setConfirmPassword] = useState('');
    const token = window.location.search.slice(7,247);
    const userId = window.location.search.slice(255);

	const handleChangePassword = event => setPassword(event.target.value);
	const handleChangeConfirmPassword = event => setConfirmPassword(event.target.value);

	const handleSubmit = event => {
		event.preventDefault();
		password !== confirmPassword
			? (errors.password = `Passwords didn't match.`)
			: onResetPassword(password, token, userId);
	};

	return (
		<>
			<div className={css['login-container']}>
				<div className='d-flex flex-column align-items-center relative'>
					<img className={css['logo']} src={Logo}></img>
					<p className={css['welcome-text']}>I'm glad you took matters into your own hands!</p>
					<p className={css['sign-in-text']}>Reset Password</p>
					<p className={css['secondary-paragraph']}>Enter a new password for </p>
					<form onSubmit={handleSubmit}>
						<div className={css['input-box']}>
							<img className={css['user-icon']} src={Password}></img>
							<input
								className={css['input-field']}
								value={password}
								onChange={handleChangePassword}
								type='password'
								placeholder='Password'
							/>
						</div>

						<div className={css['input-box']}>
							<img className={css['user-icon']} src={Password}></img>
							<input
								className={css['input-field']}
								type='password'
								value={confirmPassword}
								onChange={handleChangeConfirmPassword}
								placeholder='Confirm Password'
							/>
						</div>
						<Button className={css['submit-btn']} type='submit'>
							Submit
						</Button>
					</form>
				</div>
			</div>
		</>
	);
}
