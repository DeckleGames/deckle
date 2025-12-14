import { componentsApi } from '$lib/api';
import { error } from '@sveltejs/kit';
import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async ({ params, fetch, parent }) => {
	try {
		const parentData = await parent();
		const component = await componentsApi.getById(params.projectId, params.componentId, fetch);

		// TODO: Once we support other component types, update this validation
		if (component.type !== 'Card') {
			throw error(400, 'Component is not a card');
		}

		return {
			component,
			project: parentData.project,
			part: params.part
		};
	} catch (err) {
		console.error('Failed to load component:', err);
		throw error(404, 'Component not found');
	}
};
