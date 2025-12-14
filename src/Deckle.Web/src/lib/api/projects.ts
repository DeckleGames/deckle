import { api } from './client';
import type { Project, CreateProjectDto, UpdateProjectDto, ProjectUser } from '$lib/types';

/**
 * Projects API
 */
export const projectsApi = {
  /**
   * Get all projects for the current user
   */
  list: (fetchFn?: typeof fetch) => api.get<Project[]>('/projects', undefined, fetchFn),

  /**
   * Get a single project by ID
   */
  getById: (id: string, fetchFn?: typeof fetch) => api.get<Project>(`/projects/${id}`, undefined, fetchFn),

  /**
   * Create a new project
   */
  create: (data: CreateProjectDto, fetchFn?: typeof fetch) => api.post<Project>('/projects', data, undefined, fetchFn),

  /**
   * Update a project
   */
  update: (id: string, data: UpdateProjectDto, fetchFn?: typeof fetch) => api.put<Project>(`/projects/${id}`, data, undefined, fetchFn),

  /**
   * Get all users for a project
   */
  getUsers: (id: string, fetchFn?: typeof fetch) => api.get<ProjectUser[]>(`/projects/${id}/users`, undefined, fetchFn),

  /**
   * Delete a project
   */
  delete: (id: string, fetchFn?: typeof fetch) => api.delete(`/projects/${id}`, undefined, fetchFn),
};
