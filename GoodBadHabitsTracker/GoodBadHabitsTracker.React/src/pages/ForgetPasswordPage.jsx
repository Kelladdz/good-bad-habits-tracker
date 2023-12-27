import ForgetPassword from '../components/user/ForgetPassword';

export default function ForgetPasswordPage({ onPasswordForget, forgetPasswordErrors }) {
	return (
		<div>
			<ForgetPassword onPasswordForget={onPasswordForget} forgetPasswordErrors={forgetPasswordErrors}/>
		</div>
	);
}
