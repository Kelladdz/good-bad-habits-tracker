import css from './user.module.css';

export default function Confirm() {

	return (
		<div>
			<div className={css['login-container']}>
                <p>Look for the verification email in your inbox and click the link within it.</p>
			</div>
		</div>
	);
}
