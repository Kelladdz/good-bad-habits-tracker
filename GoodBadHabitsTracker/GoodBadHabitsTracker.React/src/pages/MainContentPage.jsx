import MainContent from '../components/main-content/MainContent';

export default function MainContentPage({onLogout}) {
	return (
		<div>
			<MainContent onLogout={onLogout}/>
		</div>
	);
}
