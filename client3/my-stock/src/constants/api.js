export const BACKEND = process.env.REACT_APP_MAIN_API_URL

const AUTH = `${BACKEND}auth/`
export const LOGIN = `${AUTH}login`
export const REGISTER = `${AUTH}register`

const SEARCH = `${BACKEND}search/`
export const SEARCHUPWARDGAP = `${SEARCH}UpwardGap`
export const SEARCHCOLSEBREAK = `${SEARCH}ColseBreak`
export const UPMA = `${SEARCH}UpMA`
export const UPMATWICE = `${SEARCH}UpMATwice`
export const SEARCHSTAARISE = `${SEARCH}STAArise`
export const APPROACH = `${SEARCH}CloseApproach`
export const CIRCULATEDMARKET = `${SEARCH}CirculatedMarket`
export const TURNOVERRATE = `${SEARCH}TurnOverRate`
export const EXCEPTZHANGFU = `${SEARCH}ExceptZhangFu`
export const NRISEOPEN = `${SEARCH}NRiseOpen`
export const VOLUMEDECREASE = `${SEARCH}VolumeDecrease`
export const VOLUMEBREAK = `${SEARCH}VolumeBreak`
export const SEARCHMACD = `${SEARCH}UpMACD`
export const MIDDLEBREAK = `${SEARCH}MiddleBreak`


//获取数据api

const VALUES = `${BACKEND}Values/`
export const GETVALUES = id=> `${VALUES}${id}`
export const GETSTOCK = id=> `${VALUES}GetStock?id=${id}`
export const GETMARGIN = `${VALUES}GetMargin`
export const GETMARKETDEAL = `${VALUES}GetMarketDeal`
export const GetStaPrice = `${VALUES}GetStaPrice`



const MANAGE = `${BACKEND}system/`
export const GETUSERS = `${MANAGE}users`
export const UPDATE_USER = userId => `${MANAGE}${userId}`;
export const DELETE_USER = `${MANAGE}`;

export const GETTASKNUM = `${MANAGE}taskNum`;
export const GETSTATUSTABLE = `${MANAGE}statusTable`;

const ABOUT= `${BACKEND}about/`
export const GETSYSTEMINFO = `${ABOUT}ServerVersion`
export const GETDATENFO = `${ABOUT}DateInfo`

//server task control

//use another server as task control service
const TASKCONTROL = `${process.env.REACT_APP_Task_API_URL}Tasks/`;
export const CLEARDATE = `${TASKCONTROL}clearDate`;
export const ariseSystemEvent = `${TASKCONTROL}ariseSystemEvent`;


export const STORIES = `${BACKEND}stories/`
export const CREATE_STORY = STORIES
export const DELETE_STORY = STORIES
export const UPDATE_STORY = storyId => `${STORIES}${storyId}`
export const PUBLISH_STORY = storyId => `${STORIES}${storyId}/publish`

export const USER_STORIES = userId => `${STORIES}user/${userId}`
export const STORY_DETAIL = storyId => `${STORIES}${storyId}`
export const DRAFTS = `${STORIES}drafts`
export const SHARED = `${STORIES}shared`
export const TOGGLE_LIKE = storyId => `${STORIES}${storyId}/toggleLike`
export const SHARE = storyId => `${STORIES}${storyId}/share`

