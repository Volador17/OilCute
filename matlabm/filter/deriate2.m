% ���ײ��
% input
% X:����Ⱦ���
% m:��ֿ��
% output
% xc:�����Ĺ���
function d=deriate2(X,m)
	[m1,n]=size(X);
	for i=1:m1-m-2
		d(i,:)=X(i+2+m,:)-2*X(i+1+m,:)+X(i,:);
	end
	d((m1-m-1):m1,:)=zeros(m+2,n);
end