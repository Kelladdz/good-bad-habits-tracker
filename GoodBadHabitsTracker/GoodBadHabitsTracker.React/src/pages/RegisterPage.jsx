import Register from '../components/user/Register';

export default function RegisterPage({onRegister}) {
	return (
		<div>
			<Register onRegister={onRegister}/>
		</div>
	);
}
