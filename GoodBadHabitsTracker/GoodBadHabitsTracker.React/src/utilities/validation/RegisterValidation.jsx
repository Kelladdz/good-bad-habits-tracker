export default function RegisterValidation(name, email, password, confirmPassword) {
	let errors = {};
	const emailPattern = /^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$/;
	const passwordPattern = /^([a-zA-Z0-9_@#$%^&*()<>?/\|}{~:.,;\-]+){6,}$/;

	if (email === '') {
		errors.email = 'Email address is required.';
	} else if (!emailPattern.test(email)) {
		errors.email = 'Email address is not correct.';
	}

	if (password.length < 6) {
		errors.password = 'Password is to short.';
	} else if (!passwordPattern.test(password)) {
		errors.password = 'Password is not correct.';
	} else if (password !== confirmPassword) errors.password = `Passwords didn't match.`;

	if (name === '') errors.name = 'Name is required.';

	return errors;
}
