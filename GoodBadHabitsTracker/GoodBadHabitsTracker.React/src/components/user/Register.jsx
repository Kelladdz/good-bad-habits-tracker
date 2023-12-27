import css from './user.module.css';
import Logo from '../../assets/logo.svg';
import User from '../../assets/user.svg';
import Email from '../../assets/email.svg';
import Password from '../../assets/password.svg';
import { Button } from 'react-bootstrap';
import Link from '../Link';
import { useEffect, useState } from 'react';
import useNavigation from '../../hooks/useNavigation';
import Validation from '../../utilities/Validation';
import axios from 'axios';

export default function Register() {
	const [name, setName] = useState('');
	const [email, setEmail] = useState('');
	const [password, setPassword] = useState('');
	const [confirmPassword, setConfirmPassword] = useState('');
	const [errors, setErrors] = useState({});
	const [serverErrors, setServerErrors] = useState('');
	const { navigate } = useNavigation();

	// const handleSubmit = () => {
	// 	event.preventDefault();
	// 	console.log(serverErrors);
	// 	onRegister(email, name, password, confirmPassword);
	// };

	const register = async (email, name, password, confirmPassword) => {
		const response = await axios
			.post('https://localhost:7154/API/Account/Register', {
				email,
				name,
				password,
				confirmPassword,
			})
			.then(res => {
				console.log(res);
				if (res.status === 201) navigate('/signin');
			})
			.catch(errs => {
				console.log(errs);
				for (let err of errs.response.data) {
					if (err.code === 'DuplicateUserName')
						setServerErrors(() => {
							if (serverErrors === '') return `This name is already taken.`;
						});
					else if (err.code === 'DuplicateEmail')
						setServerErrors(() => {
							if (serverErrors === '') return `This email is already taken.`;
						});
				}
			});
	};

	const handleSubmit = event => {
		event.preventDefault();
		setErrors(Validation(name, email, password, confirmPassword));
		setTimeout(() => {
			if (Object.keys(errors).length === 0 || (typeof errors.name === 'undefined' && email.length !== 0 && name.length !== 0 && password.length !== 0 && confirmPassword.length !== 0))
				register(email, name, password, confirmPassword);
		}, 500);
	};
	

	const handleChangeName = event => setName(event.target.value);
	const handleChangeEmail = event => setEmail(event.target.value);
	const handleChangePassword = event => setPassword(event.target.value);
	const handleChangeConfirmPassword = event => setConfirmPassword(event.target.value);

	useEffect(() => {
		return setErrors({ ...errors, [errors.name]: errors.value });
	}, [navigate]);

	useEffect(() => {
		return setServerErrors('')
	}, [errors]);

	return (
		<div>
			<div className={css['login-container']}>
				<div className='d-flex flex-column align-items-center'>
					<img className='mt-5 mb-3 w-50' src={Logo}></img>
					<p className={css['welcome-text']}>I'm glad you took matters into your own hands!</p>
					<p className={css['sign-in-text']}>Sign up</p>
					<form onSubmit={handleSubmit} style={{ height: '23rem' }}>
						<div className={css['input-box']}>
							<img className={css['user-icon']} src={User}></img>
							<input className={css['input-field']} value={name} onChange={handleChangeName} placeholder='User Name' />
						</div>
						<div style={{ marginTop: '0' }} className={css['error-box']}>
							{errors.name && <p className={css['error-text']}>{errors.name}</p>}
						</div>

						<div className={css['input-box']}>
							<img className={css['user-icon']} src={Email}></img>
							<input className={css['input-field']} value={email} onChange={handleChangeEmail} placeholder='E-mail' />
						</div>
						<div style={{ marginTop: '0' }} className={css['error-box']}>
							{errors.email && <p className={css['error-text']}>{errors.email}</p>}
							{serverErrors && <p className={css['error-text']}>{serverErrors}</p>}
						</div>

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
						<div style={{ marginTop: '0' }} className={css['error-box']}>
							{errors.password && <p className={css['error-text']}>{errors.password}</p>}
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
						<Button className={css['submit-register-btn']} style={{ bottom: 0 }} type='submit'>
							Register
						</Button>
						<div className={css['back-btn']}>
							<Link to='/signin'>Back</Link>
						</div>
					</form>
				</div>
			</div>
		</div>
	);
}
