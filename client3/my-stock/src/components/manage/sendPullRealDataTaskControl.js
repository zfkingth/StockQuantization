
import React from "react";


import { connectTo } from '../../utils/generic';
import { pullRealTimeAction } from '../../actions/manage';

import Button from '@material-ui/core/Button';




class tempcontrol extends React.PureComponent {
    constructor(props) {
        super(props);

        this.state = { info: undefined };



    }



    clickHandle = e => {
        this.props.pullRealTimeAction(
            {
                resolve: () => this.setState({ info: '服务器成功接受命令。' }),
                reject: err => this.setState({ info: err.message }),
            }
        )
    };

    render() {
        return (
            <div>
                <label>发送获取实时数据命令？</label>
                <Button  variant="contained" color="primary" onClick={this.clickHandle}>发送命令</Button>
                {this.state.info && <p>{this.state.info} </p>}
            </div>
        )
    }
}


export default connectTo(
    null,
    { pullRealTimeAction },
    tempcontrol
);
