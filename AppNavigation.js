import { createAppContainer } from 'react-navigation';
import { createStackNavigator } from 'react-navigation-stack';

import SignUp from '../screens/SignUp.js';
import ServerError from '../screens/ServerError.js';
import Login from '../screens/Login.js';
import HomeScreen from '../screens/HomeScreen.js';
import Assessment from '../screens/Assessment.js';
import AssessmentQuestions from '../screens/AssessmentQuestions.js';
import AssessmentScore from '../screens/AssessmentScore.js';
import BrainGames from '../screens/BrainGames.js';
import ActiveListening from '../screens/ActiveListening.js';
import Newsroom from '../screens/Newsroom.js';
import StartUpScreen from '../screens/StartUpScreen.js';

const AppNavigator = createStackNavigator(
  {
    StartUpScreen: {
      screen: StartUpScreen,
      navigationOptions: {
        headerShown: false
      }
    },
    Login: {
      screen: Login,
      navigationOptions: {
        headerShown: false
      }
    },
    SignUp: {
      screen: SignUp,
      navigationOptions: {
        headerShown: false
      }
    },
    HomeScreen: {
      screen: HomeScreen,
      navigationOptions: {
        headerShown: false
      }
    },
    ServerError: ServerError,
    Assessment: {
      screen: Assessment,
      navigationOptions: {
        headerShown: false
      }
    },
    AssessmentQuestions: {
      screen: AssessmentQuestions,
      navigationOptions: {
        headerShown: false
      }
    },
    AssessmentScore: {
      screen: AssessmentScore,
      navigationOptions: {
        headerShown: false
      }
    },
    BrainGames: {
      screen: BrainGames,
      navigationOptions: {
        headerShown: false
      }
    },
    ActiveListening: {
      screen: ActiveListening,
      navigationOptions: {
        headerShown: false
      }
    },
    Newsroom: {
      screen: Newsroom,
      navigationOptions: {
        headerShown: false
      }
    },
  }
);

export default createAppContainer(AppNavigator);
