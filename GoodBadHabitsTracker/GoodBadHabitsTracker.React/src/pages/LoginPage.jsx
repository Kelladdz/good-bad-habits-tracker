import Login from '../components/user/Login';

export default function LoginPage({ onLogin }) {
	return (
		<div>
			<Login onLogin={onLogin} />
		</div>
	);
}
