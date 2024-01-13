import { useEffect, useState } from 'react';
import css from './user.module.css';
import Logo from '../../assets/logo.svg';
import Password from '../../assets/password.svg';
import { Button } from 'react-bootstrap';
import useNavigation from '../../hooks/useNavigation';

export default function ResetPassword() {
	const [password, setPassword] = useState('');
	const [confirmPassword, setConfirmPassword] = useState('');
	const [hasUserData, SetHasUserData] = useState(true);
	const token = window.location.search.slice(7, 247);
	const userId = window.location.search.slice(255);
	const { navigate } = useNavigation();

	const resetPassword = async (password, token, userId) => {
		const response = await axios
			.put('https://localhost:7154/api/auth/reset-password', {
				password,
				token,
				userId,
			})
			.then(navigate('/signin'));
	};

	const handleChangePassword = event => setPassword(event.target.value);
	const handleChangeConfirmPassword = event => setConfirmPassword(event.target.value);

	const handleSubmit = event => {
		event.preventDefault();
		password !== confirmPassword
			? (errors.password = `Passwords didn't match.`)
			: resetPassword(password, token, userId);
	};

	useEffect(() => {});

	if (hasUserData)
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
	else
		return (
			<>
				<div className={css['login-container']}>
					<div className='d-flex flex-column align-items-center relative'>
						<img className={css['logo']} src={Logo}></img>
						<p className={css['welcome-text']}>I'm glad you took matters into your own hands!</p>
						<p className={css['sign-in-text']}>You shouldn't be here!</p>
						<p className={css['secondary-paragraph']}>Please leave this site.</p>
					</div>
				</div>
			</>
		);
}
