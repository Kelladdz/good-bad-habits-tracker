import useNavigation from '../hooks/use-navigation';

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
