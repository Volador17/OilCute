function [Ylast,SR,MD,nd,XScores] = PLSPredictorForANN(X,Scores, Loads, Weights, b,  Score_length, centerSpecData, centerCompValue)
% Ŀ�꣺�����ؼ��÷�
% ������������� 
%            X              ���׾���                        [�������� * δ֪�Ĳ��׸���(������)]
%            Scores         У������ͼ�÷־���               [��Ʒ��*���ɷ���]
%            Loads          У������ͼ�غɾ���               [������*���ɷ���]
%            Weights        У������ͼȨ�ؾ���               [������*���ɷ���]
%            Score_length   �÷������ĳ���                   [1 * ���ɷ���]
% �����������:	
%            XScores   δ֪�����ĵ÷־���       [ δ֪���ո��� * ���ɷָ��� ]
% ��������: ASTM ��׼E 1655-05 ��11ҳ PLS 
X = X -  centerSpecData'* ones(1, size(X,2)) ;

ColsX = size (X,2);
ColsLoads = size(Loads,2); 
Ylast = zeros (ColsX,ColsLoads);
% ��ʼ������������� ���о������Ϊk �������Ϊn
XScores =zeros (ColsX,ColsLoads);
Factor = ColsLoads;
tx = X;
% �����ǲ��� ASTM �ϵ�11ҳע�ͽ��У��ܹ�˳����ɶ� δ֪������Ԥ�� �������޸ĺ�ͨ�� Scores ���й���
for i = 1 : Factor
    Weights(:,i)=Weights(:,i)/Score_length(i);
    XScores(:,i)= X'*Weights(:,i);
    XScores(:,i)=XScores(:,i)/Score_length(i);
    MD(:,i)=diag(XScores*XScores');
    tx = tx -  Loads(:,i)*XScores(:,i)';
    SR(i,:)=(sum((tx).^2)).^0.5;
    nd(:,i)=nndr(XScores(:,1:i)',Scores(:,1:i)');
end
SR=SR';
%������ײв�

