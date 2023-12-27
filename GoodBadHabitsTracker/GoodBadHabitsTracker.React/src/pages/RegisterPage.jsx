import Register from '../components/user/Register';

export default function RegisterPage({ onRegister, registerErrors, registerServerErrors }) {
	return (
		<div>
			<Register onRegister={onRegister} registerErrors={registerErrors} registerServerErrors={registerServerErrors}/>
		</div>
	);
}
