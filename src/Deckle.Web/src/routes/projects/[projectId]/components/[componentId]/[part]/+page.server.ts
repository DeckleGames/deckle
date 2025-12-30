import { componentsApi, dataSourcesApi } from '$lib/api';
import { error } from '@sveltejs/kit';
import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async ({ params, fetch, parent }) => {
	try {
		const parentData = await parent();
		const component = await componentsApi.getById(params.projectId, params.componentId, fetch);

		if (!("dimensions" in component)) {
			throw error(400, 'Component does not have dimensions');
		}

		// Load data source details if component has one linked
		let dataSource = null;
		if (component.type === 'Card' && component.dataSource) {
			try {
				dataSource = await dataSourcesApi.getById(component.dataSource.id, fetch);
			} catch (err) {
				console.error('Failed to load data source:', err);
				// Continue without data source if it fails to load
			}
		}

		return {
			component,
			project: parentData.project,
			part: params.part,
			dataSource
		};
	} catch (err) {
		console.error('Failed to load component:', err);
		throw error(404, 'Component not found');
	}
};
