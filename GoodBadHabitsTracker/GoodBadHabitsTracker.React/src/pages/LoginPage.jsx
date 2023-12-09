import Login from '../components/user/Login';

export default function LoginPage({onLogin, onGoogleLogin}) {
	return (
		<div>
			<Login onLogin={onLogin} onGoogleLogin={onGoogleLogin}/>
		</div>
	);
}
