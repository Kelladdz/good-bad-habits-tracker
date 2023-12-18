import useNavigation from '../hooks/useNavigation';

export default function Link({ to, children }) {
	const { navigate } = useNavigation();

	const handleClick = event => {
		event.preventDefault();
		navigate(to);
	};
	return (
		<a href='#' onClick={handleClick}>
			{children}
		</a>
	);
}
