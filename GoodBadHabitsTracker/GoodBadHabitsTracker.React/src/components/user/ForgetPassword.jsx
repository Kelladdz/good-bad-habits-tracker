import css from './user.module.css';
import Logo from '../../assets/logo.svg';
import Email from '../../assets/email.svg';
import { useEffect, useState } from 'react';
import { Button } from 'react-bootstrap';

export default function ForgetPassword({ onPasswordForget, forgetPasswordErrors }) {
	const [email, setEmail] = useState('');
	const [errors, setErrors] = useState('');
	const [isSubmitted, setIsSubmitted] = useState(false);
	const handleSubmit = async event => {
		event.preventDefault();
		await onPasswordForget(email);
		setErrors(forgetPasswordErrors);
		console.log(forgetPasswordErrors);
		if (forgetPasswordErrors === undefined) setIsSubmitted(true);
	};

	const handleChangeEmail = event => {
		setEmail(event.target.value);
		console.log(email);
	};

	useEffect(() => {
		setErrors('');
	}, []);

	if (!isSubmitted) {
		return (
			<>
				<div className={css['login-container']}>
					<div className='d-flex flex-column align-items-center relative'>
						<img className={css['logo']} src={Logo}></img>
						<p className={css['welcome-text']}>I'm glad you took matters into your own hands!</p>
						<p className={css['sign-in-text']}>Forget something?</p>
						<p className={css['secondary-paragraph']}>Enter your email below to receive password reset instructions.</p>
						<form onSubmit={handleSubmit}>
							<div className={css['input-box']} style={{ marginBottom: '0' }}>
								<img className={css['user-icon']} src={Email}></img>
								<input className={css['input-field']} value={email} onChange={handleChangeEmail} placeholder='E-mail' />
							</div>
							{errors && <p style={{ color: 'red', marginBottom: '0px' }}>{errors}</p>}
							<Button className={css['submit-btn']} type='submit'>
								Submit
							</Button>
						</form>
					</div>
				</div>
			</>
		);
	} else
		return (
			<>
				<div className={css['login-container']}>
					<div className='d-flex flex-column align-items-center relative'>
						<img className={css['logo']} src={Logo}></img>
						<p className={css['welcome-text']}>I'm glad you took matters into your own hands!</p>
						<p className={css['sign-in-text']}>Check your mailbox!</p>
						<p className={css['secondary-paragraph']}>
							We send you a link to reset your password. Check your spam folder if you do not hear from us after a
							while.
						</p>
					</div>
				</div>
			</>
		);
}
