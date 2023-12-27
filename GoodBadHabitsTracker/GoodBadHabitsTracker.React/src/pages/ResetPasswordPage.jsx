import ResetPassword from '../components/user/ResetPassword';

export default function ResetPasswordPage({onResetPassword}) {
	return (
		<div>
			<ResetPassword onResetPassword={onResetPassword}/>
		</div>
	);
}
