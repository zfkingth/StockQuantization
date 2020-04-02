
import React from "react";


import { connectTo } from '../../utils/generic';
import { clearDateAction } from '../../actions/manage';
import { goToErrorPage } from '../../actions/navigation';

import Button from '@material-ui/core/Button';




class tempcontrol extends React.PureComponent {
    constructor(props) {
        super(props);

        this.state = { info: undefined };



    }



    clickHandle = e => {
        this.props.clearDateAction(
            {
                resolve: () => this.setState({ info: '服务器成功接受命令。' }),
                values: 'pullRealTime',
                reject: err => this.props.goToErrorPage(err),

            }
        )
    };

    render() {
        return (
            <div>
                <label>发送清除实时数据事件状态的命令？</label>
                <Button variant="contained" color="primary" onClick={this.clickHandle}>发送命令</Button>
                {this.state.info && <p>{this.state.info} </p>}
            </div>
        )
    }
}


export default connectTo(
    null,
    { clearDateAction,goToErrorPage },
    tempcontrol
);
