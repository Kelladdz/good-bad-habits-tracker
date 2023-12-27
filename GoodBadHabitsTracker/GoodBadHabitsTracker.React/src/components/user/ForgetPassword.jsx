import css from './user.module.css';
import Logo from '../../assets/logo.svg';
import Email from '../../assets/email.svg';
import Link from '../Link';
import { useEffect, useState } from 'react';
import { Button } from 'react-bootstrap';
import ForgetPasswordValidation from '../../utilities/validation/ForgetPasswordValidation';
import useNavigation from '../../hooks/useNavigation';

export default function ForgetPassword() {
	const [email, setEmail] = useState('');
	const [errors, setErrors] = useState({});
	const [serverErrors, setServerErrors] = useState('');
	const [isSubmitted, setIsSubmitted] = useState(false);
	const { navigate } = useNavigation();

	const sendLinkForResetPassword = async email => {
		const response = await axios
			.post('https://localhost:7154/API/Account/ForgetPassword', {
				email,
			})
			.then(res => {
				console.log(res.data);
				setIsSubmitted(true);
			})
			.catch(errs => {
				console.log(errs);
				if (errs.response.status === 404)
					setServerErrors(() => {
						if (serverErrors === '' && Object.keys(errors).length === 0 && email.length !== 0)
							return `This account doesn't exist.`;
					});
			});
		console.log(serverErrors);
	};

	const handleSubmit = event => {
		event.preventDefault();
		setErrors(ForgetPasswordValidation(email));
		setTimeout(() => {
			if (
				((Object.keys(errors).length === 0 || typeof Object.keys(errors) === undefined) && email.length !== 0) 
			)
				sendLinkForResetPassword(email);
		}, 1000);
	};

	const handleChangeEmail = event => {
		setEmail(event.target.value);
		console.log(email);
	};

	useEffect(() => {
		return setServerErrors('');
	}, [errors]);

	if (!isSubmitted) {
		return (
			<>
				<div className={css['login-container']}>
					<div className='d-flex flex-column align-items-center relative'>
						<img className={css['logo']} src={Logo}></img>
						<p className={css['welcome-text']}>I'm glad you took matters into your own hands!</p>
						<p className={css['sign-in-text']}>Forgot something?</p>
						<p className={css['secondary-paragraph']}>Enter your email below to receive password reset instructions.</p>
						<form onSubmit={handleSubmit}>
							<div className={css['input-box']} style={{ marginBottom: '0' }}>
								<img className={css['user-icon']} src={Email}></img>
								<input className={css['input-field']} value={email} onChange={handleChangeEmail} placeholder='E-mail' />
							</div>
							{errors.email && <p style={{ color: 'red', marginBottom: '0px' }}>{errors.email}</p>}
							{serverErrors && <p style={{ color: 'red', marginBottom: '0px' }}>{serverErrors}</p>}
							<Button className={css['submit-btn']} type='submit'>
								Submit
							</Button>
							<div className={css['back-btn']} style={{ bottom: '-2rem' }}>
								<Link to='/signin'>Back</Link>
							</div>
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
