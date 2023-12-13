import css from './user.module.css';
import Logo from '../../assets/logo.svg';
import User from '../../assets/user.svg';
import Password from '../../assets/password.svg';
import { Button } from 'react-bootstrap';
import Link from '../Link';
import { useState } from 'react';
import useNavigation from '../../hooks/use-navigation';


export default function Register({onRegister}) {
	const [email, setEmail] = useState('');
	const [password, setPassword] = useState('');
	const [submitted, setSubmitted] = useState(false);
	const {navigate} = useNavigation();
	const handleSubmit = event => {
		event.preventDefault();		
		setSubmitted(true);
		onRegister(email, password);		
	};

	const handleChangeEmail = event => setEmail(event.target.value);
	const handleChangePassword = event => setPassword(event.target.value);

	if(!submitted){
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
								<input className={css['input-field']} value={email} onChange={handleChangeEmail} placeholder='E-mail' />
							</div>
							<div className={css['input-box']}>
								<img className={css['user-icon']} src={Password}></img>
								<input className={css['input-field']} value={password} onChange={handleChangePassword} type='password' placeholder='Password' />
							</div>
							<div className={css['input-box']}>
								<img className={css['user-icon']} src={Password}></img>
								<input className={css['input-field']} type='password' placeholder='Confirm Password' />
							</div>
							<Button className={css['submit-btn']} type='submit' >Register</Button>
							<div className={css['register-btn']}>
								<Link to='/signin'>Back</Link>
							</div>
						</form>
					</div>
				</div>
			</div>
			);
		}
	else return (
		<div className={css['login-container']}>
                <p>Look for the verification email in your inbox and click the link within it.</p>
			</div>
	)
}
