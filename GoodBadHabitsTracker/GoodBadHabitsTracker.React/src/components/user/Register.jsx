import css from './user.module.css';
import Logo from '../../assets/logo.svg';
import User from '../../assets/user.svg';
import Email from '../../assets/email.svg';
import Password from '../../assets/password.svg';
import { Button } from 'react-bootstrap';
import Link from '../Link';
import { useEffect, useState } from 'react';
import useNavigation from '../../hooks/use-navigation';
import Validation from '../../utilities/Validation';

export default function Register({ onRegister, catches }) {
	const [name, setName] = useState('');
	const [email, setEmail] = useState('');
	const [password, setPassword] = useState('');
	const [confirmPassword, setConfirmPassword] = useState('');
	const [errors, setErrors] = useState({});
	const { navigate } = useNavigation();

	const handleSubmit = () => {
		onRegister(email, name, password, confirmPassword);
	};

	function handleValidation(event) {
		event.preventDefault();
		const validationErrors = Validation(name, email, password, confirmPassword);
		setErrors(validationErrors);
		if (Object.keys(errors).length === 0) handleSubmit();
	}

	let emailIsTakenInfo = null;
	if (catches.email !== null) {
		emailIsTakenInfo = <p style={{ color: 'red', marginBottom: '0px' }}>{catches.email}</p>;
	}
	let nameIsTakenInfo = null;
	if (catches.name !== null) {
		nameIsTakenInfo = <p style={{ color: 'red', marginBottom: '0px' }}>{catches.name}</p>;
	}

	const handleChangeName = event => setName(event.target.value);
	const handleChangeEmail = event => setEmail(event.target.value);
	const handleChangePassword = event => setPassword(event.target.value);
	const handleChangeConfirmPassword = event => setConfirmPassword(event.target.value);

	useEffect(() => {
		setErrors({ ...errors, [catches.name]: catches.value });
	}, []);

	return (
		<div>
			<div className={css['login-container']}>
				<div className='d-flex flex-column align-items-center'>
					<img className='mt-5 mb-3 w-50' src={Logo}></img>
					<p className={css['welcome-text']}>I'm glad you took matters into your own hands!</p>
					<p className={css['sign-in-text']}>Sign up</p>
					<form onSubmit={handleValidation}>
						<div className={css['input-box']}>
							<img className={css['user-icon']} src={User}></img>
							<input className={css['input-field']} value={name} onChange={handleChangeName} placeholder='User Name' />
						</div>
						{errors.name && <p style={{ color: 'red', marginBottom: '0px' }}>{errors.name}</p>}
						{nameIsTakenInfo}
						<div className={css['input-box']}>
							<img className={css['user-icon']} src={Email}></img>
							<input className={css['input-field']} value={email} onChange={handleChangeEmail} placeholder='E-mail' />
						</div>
						{errors.email && <p style={{ color: 'red', marginBottom: '0px' }}>{errors.email}</p>}
						{emailIsTakenInfo}
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
						{errors.password && <p style={{ color: 'red', marginBottom: '0px' }}>{errors.password}</p>}
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
							Register
						</Button>
						<div className={css['register-btn']}>
							<Link to='/signin'>Back</Link>
						</div>
					</form>
				</div>
			</div>
		</div>
	);
}
