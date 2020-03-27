import { createReducer } from 'redux-act'

import * as a from '../actions/cache'

export const getDefaultState = _ => ({
  stateReceived: {
    login: true,
    register: true,
    editor: true,
    yourStories: false,
    story: false,
    stories: false
  }
})

// const changeStateReceived = (state, page, value) => ({
//   ...state,
//   stateReceived: {
//     ...state.stateReceived,
//     [page]: value
//   }
// })


export default _ =>
  createReducer(
    {
      // only for sagas usage
      [a.updateState]: (state, newState) => ({ ...state, ...newState }),
      [a.saveCache]: (state, { page, projectId, pageState }) => ({
        ...state,
        stateReceived: {
          ...state.stateReceived,
          [page]: false
        },
        [page]: {
          ...state[page],
          [projectId]: pageState
        },
      }),
      [a.removeStateReceivedFrom]: (state, page) => ({
        ...state,
        stateReceived: {
          ...state.stateReceived,
          [page]: false
        }
      }),
      // [receiveStoriesForTab]: state => changeStateReceived(state, 'yourStories', true),
      // [clearYourStories]: state => changeStateReceived(state, 'yourStories', false),
      // [receiveStory]: state => changeStateReceived(state, 'story', true),
      // [receiveStories]: state => changeStateReceived(state, 'stories', true),
    },
    getDefaultState()
  )
