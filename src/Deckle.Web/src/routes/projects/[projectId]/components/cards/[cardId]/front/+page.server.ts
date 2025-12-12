import { componentsApi } from '$lib/api';
import { error } from '@sveltejs/kit';
import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async ({ params, fetch }) => {
  try {
    const component = await componentsApi.getById(params.projectId, params.cardId, fetch);

    if (component.type !== 'Card') {
      throw error(400, 'Component is not a card');
    }

    return {
      component,
      project: {
        id: params.projectId
      }
    };
  } catch (err) {
    console.error('Failed to load component:', err);
    throw error(404, 'Component not found');
  }
};
