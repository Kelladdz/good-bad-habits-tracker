export default function ForgetPasswordValidation(email) {
	let errors = {};
	const emailPattern = /^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$/;

	if (email === '') {
		errors.email = 'Email address is required.';
	} else if (!emailPattern.test(email)) {
		errors.email = 'Email address is not correct.';
	}

	return errors;
}
