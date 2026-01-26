import { error, redirect } from '@sveltejs/kit';
import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async ({ parent }) => {
	const { user } = await parent();

	// Redirect to login if not authenticated
	if (!user) {
		throw redirect(302, '/');
	}

	// Check if user has administrator role
	if (user.role !== 'Administrator') {
		throw error(403, 'Access denied. Administrator privileges required.');
	}

	return {
		user
	};
};
