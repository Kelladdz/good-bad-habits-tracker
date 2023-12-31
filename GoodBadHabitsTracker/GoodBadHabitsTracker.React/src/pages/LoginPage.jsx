import Login from '../components/user/Login';
import { ErrorsProvider } from '../context/errors';

export default function LoginPage({ onGoogleLogin }) {
	return (
		<div>
			<ErrorsProvider>
				<Login onGoogleLogin={onGoogleLogin} />
			</ErrorsProvider>
		</div>
	);
}
