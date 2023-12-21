import Login from '../components/user/Login';
import { ErrorsProvider } from '../context/errors';

export default function LoginPage({ onLogin, loginErrors }) {
	return (
		<div>
			<ErrorsProvider>
				<Login onLogin={onLogin} loginErrors={loginErrors} />
			</ErrorsProvider>
		</div>
	);
}
