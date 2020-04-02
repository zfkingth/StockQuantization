import { createReducer } from 'redux-act'
import * as a from '../actions/navigation'

import { unauthorizeUser } from '../actions/auth'
import { loggedIn } from '../utils/auth'
import {transformError} from '../utils/api'

const getDefaultState = page => ({
  page,
  storyId: undefined,
  errorInfo: undefined
})

const forward = (state, page) => ({ ...state, page })

export default _ =>
  createReducer(
    {
      [a.to]: forward,
      [a.toStory]: (state, storyId) => ({ ...state, page: 'story', storyId }),
      [a.goToErrorPage]: (state, errorInfo) => ({ 
        ...state, 
        page: 'errorPage',
       errorInfo: transformError( errorInfo) }),
      [unauthorizeUser]: state => forward(state, 'login'),
    },
    getDefaultState(process.env.REACT_APP_MOCK
      ? undefined
      : loggedIn() ? 'filterCloseBreak' : 'login'
    )
  )
