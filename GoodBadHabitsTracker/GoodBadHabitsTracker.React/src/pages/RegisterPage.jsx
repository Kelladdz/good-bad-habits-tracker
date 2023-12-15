import Register from '../components/user/Register';

export default function RegisterPage({ onRegister, errors }) {
	return (
		<div>
			<Register onRegister={onRegister} catches={errors} />
		</div>
	);
}
