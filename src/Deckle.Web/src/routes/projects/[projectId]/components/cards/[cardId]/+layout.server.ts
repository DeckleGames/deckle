import { componentsApi } from '$lib/api';
import { error } from '@sveltejs/kit';
import type { LayoutServerLoad } from './$types';

export const load: LayoutServerLoad = async ({ params, fetch, parent }) => {
	try {
		const parentData = await parent();
		const component = await componentsApi.getById(params.projectId, params.cardId, fetch);

		if (component.type !== 'Card') {
			throw error(400, 'Component is not a card');
		}

		return {
			component,
			project: parentData.project
		};
	} catch (err) {
		console.error('Failed to load component:', err);
		throw error(404, 'Component not found');
	}
};
